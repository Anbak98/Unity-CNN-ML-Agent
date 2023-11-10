from flask import Flask, request, jsonify, make_response
import os
import torch, gc
import torch.nn as nn
import numpy as np
import segmentation_models_pytorch as smp
from PIL import Image
import io
from io import BytesIO

app = Flask(__name__)

app.config['UPLOAD_FOLDER'] = 'C:\\Users\\copom\\Artificial_Hunter_Vision\\SegmentationModel\\ProcessedImg\\'

mean = torch.tensor([0.3230084478855133, 0.3013632297515869, 0.29226961731910706, 0.6981670260429382]).view(4,1,1)
std = torch.tensor([0.10305266082286835, 0.06567802280187607, 0.13611936569213867, 0.4313824474811554]).view(4,1,1)

# 모델
model = smp.Unet(
    encoder_name="mobilenet_v2",
    in_channels=4,
    classes=5,
    encoder_depth=4,
    decoder_channels = [256,128,64,32],
)
device = "cuda" if torch.cuda.is_available() else "cpu"
model = model.to(device)

# 로드하기
model_pts = ["_100_optimal_segmentcheckpoint.pt"]
ptdir = "C:\\Users\\copom\\Artificial_Hunter_Vision\\SegmentationModel\\PT\\"

# 파라미터 로드
checkpoint = torch.load(os.path.join(ptdir, "_100_optimal_segmentcheckpoint.pt"), map_location=torch.device(device))
model.load_state_dict(checkpoint)

def do_predict(image):
    img = Image.open(io.BytesIO(image))
    input_np = np.array(img)
    # print(f"shape: {input_np.shape}")

    input_tensor = torch.from_numpy(input_np).float()
    input_tensor = input_tensor.permute(2, 0, 1)
    # print(f"input_tensor shape: {input_tensor.shape}")
    input_tensor = (input_tensor - mean) / std  # 학습에 사용한 통계로 정규화
    
    # print(f"input_tensor shape: {input_tensor.shape}")
    output = model(input_tensor.unsqueeze(0))
    
    # print(f"output shape: {output.shape}")
    prediction = output.argmax(dim = 1, keepdim = False) # 모델의 출력으로부터 segmantation map 도출, 현재 (1, 64, 64) 크기의 텐서
    # print(f"output shape: {output.shape}")
    sky, wall, floor, box, target = range(5)
    rgb = {
            sky   : (67,116,217),   # blue
            wall  : (100,100,100),  # gray
            floor : (25,25,25),     # black
            box   : (236,239,240),    # yellow
            target: (190,58,39)     # green
          }
    label = prediction.squeeze()
    map = torch.zeros((64,64,3))
    for l in range(5):
        map += (label == l).view(64,64,1) * torch.tensor(rgb[l]).view(1,-1)
    map = map.int()
    # print(f"map shape: {map.shape}")

    label_map = map.detach().numpy()
    # print(f"label_map shape: {label_map.shape}")
    label_map = label_map.astype(np.float32)
    label_map = Image.fromarray(np.uint8(label_map))
    
    results = BytesIO()
    label_map.save(results, format='png')
    return results.getvalue()

@app.route('/upload', methods=['POST'])
def upload_image():
    # POST 요청에서 이미지 데이터 가져오기
		# byte[]
    image_data = request.files['image'].read()


    
    # img = Image.open(BytesIO(image_data))
    # buffered = BytesIO()
    # img.save(buffered, format="PNG")
    # restored_image_data = buffered.getvalue()
    # print(f"restored len: {len(restored_image_data)}")
		# # image_data = np.array(raw_data).reshape(64, 64, 4)

		# 이미지 처리 
    if image_data != None:
      processed_image = do_predict(image_data)
			# # 혹시 저장할 필요가 있을 때 사용
      # filename = generate_unique_filename()
      # processed_image.save(os.path.join(app.config['UPLOAD_FOLDER'], "img"))
			# 응답
      response = make_response(processed_image)
      response.headers['Content-Type'] = 'application/octet-stream'
      return response

    return "Invalid file format."

if __name__ == '__main__':
    # 개발 서버 실행
    app.run(debug=True, port=5555)
   